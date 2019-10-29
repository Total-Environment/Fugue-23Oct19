import { GeneralTermsAndConditions } from '../../src/components/checklists/general_terms_and_conditions';
import chai, {expect} from 'chai';
import React from 'react';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';

chai.use(chaiEnzyme());

describe('GeneralTermsAndConditions', () => {
    let props;
    let wrapper;

    beforeEach(() => {
        props = {
            details: {
                checkListId: 'SRID0001',
                id: '23',
                title: 'GENERAL',
                template: 'QuantityEvaluationMethod',
                content: {
                    entries: [{
                        cells: [{
                            value: 'S.No',
                            key: '0 0'
                        }],
                        key: 0
                    },
                        {
                            cells: [{
                                value: '1',
                                key: '1 0'
                            }],
                            key: 1
                        }]
                }
            },
        };
        wrapper = shallow(<GeneralTermsAndConditions {...props}/>);
    });

    it('should render error message if entries in details are not found', () => {
        props.details.content.entries = null;
        wrapper = shallow(<GeneralTermsAndConditions {...props}/>);
        const result = wrapper.find('div');
        expect(result.contains('GeneralTermsAndConditions checklist data is not found')).to.equal(true);
    });

    it('should render header data and content', () => {
        expect(wrapper).to.contain(<b>GENERAL</b>);
        const result = wrapper.find('TermsAndConditionsContent');
        expect(result).to.have.prop('entries').deep.equal(props.details.content.entries);
    });
});
