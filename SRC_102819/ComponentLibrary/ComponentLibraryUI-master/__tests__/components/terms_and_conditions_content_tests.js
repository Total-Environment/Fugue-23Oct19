import { TermsAndConditionsContent } from '../../src/components/checklists/terms_and_conditions_content';
import chai, {expect} from 'chai';
import React from 'react';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';

chai.use(chaiEnzyme());

describe('Terms and conditions content', () => {
    let props;
    let wrapper;

    beforeEach(() => {
        props = {
            entries: [{
                cells: [{
                    key: "0 0",
                    value: "S.No"
                },
                    {
                        key: "0 1",
                        value: "Header"
                    },
                    {
                        key: "0 2",
                        value: "Sub Header"
                    }],
                key: 0
            },
                {
                    cells: [{
                        key: "1 0",
                        value: "1"
                    },
                        {
                            key: "1 1",
                            value: "termination"
                        },
                        {
                            key: "1 2",
                            value: "details"
                        }
                        ],
                    key: 1
                }]
        };

        wrapper = shallow(<TermsAndConditionsContent {...props}/>);
    });

    it('should render only two columns when subHeader is not there in entry', () => {
        props = {
            entries: [{
                    cells: [{
                        key: "1 0",
                        value: "1"
                    },
                        {
                            key: "1 1",
                            value: "termination"
                        },
                        {
                            key: "1 2"
                        }
                    ],
                    key: 1
                }]
        };

        wrapper = shallow(<TermsAndConditionsContent {...props}/>);

        const result = wrapper.instance().renderMergeCells(props.entries[0]);
        expect(result[0][0].type).to.equal('td');
        expect(result[0][1].type).to.equal('td');
        expect(result[0][2]).to.equal(undefined);
    });

    it('should render three columns when subHeader is exist', () => {
        const result = wrapper.instance().renderMergeCells(props.entries[1]);
        expect(result[0][0].type).to.equal('td');
        expect(result[0][1].type).to.equal('td');
        expect(result[0][2]).to.not.equal(undefined);
        expect(result[0][2].type).to.equal('td');
    });

    it('renderCell should return cell value with type td', () => {
        const result = wrapper.instance().renderCell(props.entries[1].cells[0]);
        expect(result.type).to.equal('td');
        expect(result.props.children).to.equal('1');
    });

    it('renderCell should return undefined', () => {
        props.entries[1].cells[0] =  {
            key: "1 0"
        };
        const result = wrapper.instance().renderCell(props.entries[1].cells[0]);
        expect(result).to.equal(undefined);
    });

    it('renderEntry should return list of cells', () => {
        const result = wrapper.instance().renderEntry(props.entries[1]);
        expect(result.type).to.equal('tr');
        expect(result.props.children[0][0].props.children).to.equal('1');
        expect(result.props.children[0][0].type).to.equal('td');
    });
});
