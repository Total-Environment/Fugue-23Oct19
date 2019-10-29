import { SpecialTermsAndConditions } from '../../src/components/checklists/special_terms_and_conditions';
import chai, {expect} from 'chai';
import React from 'react';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';

chai.use(chaiEnzyme());

describe('SpecialTermsAndConditions', () => {
    let props;
    let wrapper;

    beforeEach(() => {
        props = {
            details: {
                checkListId: 'SRID0001',
                id: '23',
                title: 'Specific PO',
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
        wrapper = shallow(<SpecialTermsAndConditions {...props}/>);
    });

    it('should render error message if entries in details are not found', () => {
        props.details.content.entries = null;
        wrapper = shallow(<SpecialTermsAndConditions {...props}/>);
        const result = wrapper.find('div');
        expect(result.contains('SpecialTermsAndConditions checklist data is not found')).to.equal(true);
    });

    it('should render header data and content', () => {
        expect(wrapper).to.contain(<b>Specific PO</b>);
        const result = wrapper.find('TermsAndConditionsContent');
        expect(result).to.have.prop('entries').deep.equal(props.details.content.entries);
    });
});
